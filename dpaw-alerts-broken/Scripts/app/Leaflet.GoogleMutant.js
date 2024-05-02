L.GridLayer.GoogleMutant = L.GridLayer.extend({
    options: { minZoom: 0, maxZoom: 23, tileSize: 256, subdomains: "abc", errorTileUrl: "", attribution: "", opacity: 1, continuousWorld: false, noWrap: false, type: "roadmap", maxNativeZoom: 21 },
    initialize: function (options) {
        L.GridLayer.prototype.initialize.call(this, options);
        this._ready = !!window.google && !!window.google.maps && !!window.google.maps.Map;
        this._GAPIPromise = this._ready
            ? Promise.resolve(window.google)
            : new Promise(function (resolve, reject) {
                  var checkCounter = 0;
                  var intervalId = null;
                  intervalId = setInterval(function () {
                      if (checkCounter >= 10) {
                          clearInterval(intervalId);
                          return reject(new Error("window.google not found after 10 attempts"));
                      }
                      if (!!window.google && !!window.google.maps && !!window.google.maps.Map) {
                          clearInterval(intervalId);
                          return resolve(window.google);
                      }
                      checkCounter++;
                  }, 500);
              });
        this._tileCallbacks = {};
        this._freshTiles = {};
        this._imagesPerTile = this.options.type === "hybrid" ? 2 : 1;
    },
    onAdd: function (map) {
        L.GridLayer.prototype.onAdd.call(this, map);
        this._initMutantContainer();
        this._GAPIPromise.then(
            function () {
                this._ready = true;
                this._map = map;
                this._initMutant();
                map.on("viewreset", this._reset, this);
                map.on("move", this._update, this);
                map.on("zoomend", this._handleZoomAnim, this);
                map.on("resize", this._resize, this);
                google.maps.event.addListenerOnce(
                    this._mutant,
                    "idle",
                    function () {
                        this._checkZoomLevels();
                        this._mutantIsReady = true;
                    }.bind(this)
                );
                map._controlCorners.bottomright.style.marginBottom = "20px";
                map._controlCorners.bottomleft.style.marginBottom = "20px";
                this._reset();
                this._update();
                if (this._subLayers) {
                    for (var layerName in this._subLayers) {
                        this._subLayers[layerName].setMap(this._mutant);
                    }
                }
            }.bind(this)
        );
    },
    onRemove: function (map) {
        L.GridLayer.prototype.onRemove.call(this, map);
        map._container.removeChild(this._mutantContainer);
        this._mutantContainer = undefined;
        google.maps.event.clearListeners(map, "idle");
        google.maps.event.clearListeners(this._mutant, "idle");
        map.off("viewreset", this._reset, this);
        map.off("move", this._update, this);
        map.off("zoomend", this._handleZoomAnim, this);
        map.off("resize", this._resize, this);
        if (map._controlCorners) {
            map._controlCorners.bottomright.style.marginBottom = "0em";
            map._controlCorners.bottomleft.style.marginBottom = "0em";
        }
    },
    getAttribution: function () {
        return this.options.attribution;
    },
    setOpacity: function (opacity) {
        this.options.opacity = opacity;
        if (opacity < 1) {
            L.DomUtil.setOpacity(this._mutantContainer, opacity);
        }
    },
    setElementSize: function (e, size) {
        e.style.width = size.x + "px";
        e.style.height = size.y + "px";
    },
    addGoogleLayer: function (googleLayerName, options) {
        if (!this._subLayers) this._subLayers = {};
        return this._GAPIPromise.then(
            function () {
                var Constructor = google.maps[googleLayerName];
                var googleLayer = new Constructor(options);
                googleLayer.setMap(this._mutant);
                this._subLayers[googleLayerName] = googleLayer;
                return googleLayer;
            }.bind(this)
        );
    },
    removeGoogleLayer: function (googleLayerName) {
        var googleLayer = this._subLayers && this._subLayers[googleLayerName];
        if (!googleLayer) return;
        googleLayer.setMap(null);
        delete this._subLayers[googleLayerName];
    },
    _initMutantContainer: function () {
        if (!this._mutantContainer) {
            this._mutantContainer = L.DomUtil.create("div", "leaflet-google-mutant leaflet-top leaflet-left");
            this._mutantContainer.id = "_MutantContainer_" + L.Util.stamp(this._mutantContainer);
            this._mutantContainer.style.zIndex = "800";
            this._mutantContainer.style.pointerEvents = "none";
            this._map.getContainer().appendChild(this._mutantContainer);
        }
        this.setOpacity(this.options.opacity);
        this.setElementSize(this._mutantContainer, this._map.getSize());
        this._attachObserver(this._mutantContainer);
    },
    _initMutant: function () {
        if (!this._ready || !this._mutantContainer) return;
        this._mutantCenter = new google.maps.LatLng(0, 0);
        var map = new google.maps.Map(this._mutantContainer, {
            center: this._mutantCenter,
            zoom: 0,
            tilt: 0,
            mapTypeId: this.options.type,
            disableDefaultUI: true,
            keyboardShortcuts: false,
            draggable: false,
            disableDoubleClickZoom: true,
            scrollwheel: false,
            streetViewControl: false,
            styles: this.options.styles || {},
            backgroundColor: "transparent",
        });
        this._mutant = map;
        google.maps.event.addListenerOnce(
            map,
            "idle",
            function () {
                var nodes = this._mutantContainer.querySelectorAll("a");
                for (var i = 0; i < nodes.length; i++) {
                    nodes[i].style.pointerEvents = "auto";
                }
            }.bind(this)
        );
        this.fire("spawned", { mapObject: map });
    },
    _attachObserver: function _attachObserver(node) {
        var observer = new MutationObserver(this._onMutations.bind(this));
        observer.observe(node, { childList: true, subtree: true });
    },
    _onMutations: function _onMutations(mutations) {
        for (var i = 0; i < mutations.length; ++i) {
            var mutation = mutations[i];
            for (var j = 0; j < mutation.addedNodes.length; ++j) {
                var node = mutation.addedNodes[j];
                if (node instanceof HTMLImageElement) {
                    this._onMutatedImage(node);
                } else if (node instanceof HTMLElement) {
                    Array.prototype.forEach.call(node.querySelectorAll("img"), this._onMutatedImage.bind(this));
                }
            }
        }
    },
    _roadRegexp: /!1i(\d+)!2i(\d+)!3i(\d+)!/,
    _satRegexp: /x=(\d+)&y=(\d+)&z=(\d+)/,
    _staticRegExp: /StaticMapService\.GetMapImage/,
    _onMutatedImage: function _onMutatedImage(imgNode) {
        var coords;
        var match = imgNode.src.match(this._roadRegexp);
        var sublayer = 0;
        if (match) {
            coords = { z: match[1], x: match[2], y: match[3] };
            if (this._imagesPerTile > 1) {
                imgNode.style.zIndex = 1;
                sublayer = 1;
            }
        } else {
            match = imgNode.src.match(this._satRegexp);
            if (match) {
                coords = { x: match[1], y: match[2], z: match[3] };
            }
            sublayer = 0;
        }
        if (coords) {
            var tileKey = this._tileCoordsToKey(coords);
            imgNode.style.position = "absolute";
            imgNode.style.visibility = "hidden";
            var key = tileKey + "/" + sublayer;
            this._freshTiles[key] = imgNode;
            if (key in this._tileCallbacks && this._tileCallbacks[key]) {
                this._tileCallbacks[key].pop()(imgNode);
                if (!this._tileCallbacks[key].length) {
                    delete this._tileCallbacks[key];
                }
            } else {
                if (this._tiles[tileKey]) {
                    var c = this._tiles[tileKey].el;
                    var oldImg = sublayer === 0 ? c.firstChild : c.firstChild.nextSibling;
                    var cloneImgNode = this._clone(imgNode);
                    c.replaceChild(cloneImgNode, oldImg);
                }
            }
        } else if (imgNode.src.match(this._staticRegExp)) {
            imgNode.style.visibility = "hidden";
        }
    },
    createTile: function (coords, done) {
        var key = this._tileCoordsToKey(coords);
        var tileContainer = L.DomUtil.create("div");
        tileContainer.dataset.pending = this._imagesPerTile;
        done = done.bind(this, null, tileContainer);
        for (var i = 0; i < this._imagesPerTile; i++) {
            var key2 = key + "/" + i;
            if (key2 in this._freshTiles) {
                var imgNode = this._freshTiles[key2];
                tileContainer.appendChild(this._clone(imgNode));
                tileContainer.dataset.pending--;
            } else {
                this._tileCallbacks[key2] = this._tileCallbacks[key2] || [];
                this._tileCallbacks[key2].push(
                    function (c) {
                        return function (imgNode) {
                            c.appendChild(this._clone(imgNode));
                            c.dataset.pending--;
                            if (!parseInt(c.dataset.pending)) {
                                done();
                            }
                        }.bind(this);
                    }.bind(this)(tileContainer)
                );
            }
        }
        if (!parseInt(tileContainer.dataset.pending)) {
            L.Util.requestAnimFrame(done);
        }
        return tileContainer;
    },
    _clone: function (imgNode) {
        var clonedImgNode = imgNode.cloneNode(true);
        clonedImgNode.style.visibility = "visible";
        return clonedImgNode;
    },
    _checkZoomLevels: function () {
        var zoomLevel = this._map.getZoom();
        var gMapZoomLevel = this._mutant.getZoom();
        if (!zoomLevel || !gMapZoomLevel) return;
        if (gMapZoomLevel !== zoomLevel || gMapZoomLevel > this.options.maxNativeZoom) {
            this._setMaxNativeZoom(gMapZoomLevel);
        }
    },
    _setMaxNativeZoom: function (zoomLevel) {
        if (zoomLevel != this.options.maxNativeZoom) {
            this.options.maxNativeZoom = zoomLevel;
            this._resetView();
        }
    },
    _reset: function () {
        this._initContainer();
    },
    _update: function () {
        if (this._mutant) {
            var center = this._map.getCenter();
            var _center = new google.maps.LatLng(center.lat, center.lng);
            this._mutant.setCenter(_center);
            var zoom = this._map.getZoom();
            var fractionalLevel = zoom !== Math.round(zoom);
            var mutantZoom = this._mutant.getZoom();
            if (!fractionalLevel && zoom != mutantZoom) {
                this._mutant.setZoom(zoom);
                if (this._mutantIsReady) this._checkZoomLevels();
            }
        }
        L.GridLayer.prototype._update.call(this);
    },
    _resize: function () {
        var size = this._map.getSize();
        if (this._mutantContainer.style.width === size.x && this._mutantContainer.style.height === size.y) return;
        this.setElementSize(this._mutantContainer, size);
        if (!this._mutant) return;
        google.maps.event.trigger(this._mutant, "resize");
    },
    _handleZoomAnim: function () {
        if (!this._mutant) return;
        var center = this._map.getCenter();
        var _center = new google.maps.LatLng(center.lat, center.lng);
        this._mutant.setCenter(_center);
        this._mutant.setZoom(Math.round(this._map.getZoom()));
    },
    _removeTile: function (key) {
        if (!this._mutant) return;
        setTimeout(this._pruneTile.bind(this, key), 1000);
        return L.GridLayer.prototype._removeTile.call(this, key);
    },
    _pruneTile: function (key) {
        var gZoom = this._mutant.getZoom();
        var tileZoom = key.split(":")[2];
        var googleBounds = this._mutant.getBounds();
        var sw = googleBounds.getSouthWest();
        var ne = googleBounds.getNorthEast();
        var gMapBounds = L.latLngBounds([
            [sw.lat(), sw.lng()],
            [ne.lat(), ne.lng()],
        ]);
        for (var i = 0; i < this._imagesPerTile; i++) {
            var key2 = key + "/" + i;
            if (key2 in this._freshTiles) {
                var tileBounds = this._map && this._keyToBounds(key);
                var stillVisible = this._map && tileBounds.overlaps(gMapBounds) && tileZoom == gZoom;
                if (!stillVisible) delete this._freshTiles[key2];
            }
        }
    },
});
L.gridLayer.googleMutant = function (options) {
    return new L.GridLayer.GoogleMutant(options);
};
