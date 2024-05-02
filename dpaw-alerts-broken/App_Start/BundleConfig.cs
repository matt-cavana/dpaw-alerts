using System.Web;
using System.Web.Optimization;

namespace dpaw_alerts
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new Bundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*" /*, "~/Scripts/additional-methods.min.js", "~/Scripts/globalize.js").IncludeDirectory("~/Scripts/globalize/", "*.js"*/));

            //custom bundle for theme
            bundles.Add(new Bundle("~/bundles/customjs").Include(
                "~/Scripts/pace.min.js", "~/Scripts/jquery.resize.min.js", "~/Scripts/jquery.dataTables.min.js",  "~/Scripts/jquery.slimscroll.min.js", "~/Scripts/jquery.popupoverlay.min.js", 
                "~/Scripts/chosen.jquery.min.js",
                "~/Scripts/jquery.cookie.min.js", "~/Scripts/jquery.colorbox.min.js", /* "~/Scripts/app/app_dashboard.js", */ "~/Scripts/app/app.js", "~/Scripts/summernote.js"));

            

            //custom bundle for theme
            bundles.Add(new Bundle("~/bundles/date").Include("~/Scripts/moment.js", "~/Scripts/jquery.datetimepicker.full.js"));

            //custom bundle for theme
            bundles.Add(new Bundle("~/bundles/form").Include("~/Scripts/bootstrap-datepicker.min.js", "~/Scripts/bootstrap-timepicker.min.js", "~/Scripts/bootstrap-slider.min.js",
                "~/Scripts/jquery.tagsinput.min.js", "~/Scripts/jquery.maskedinput.min.js", "~/Scripts/jquery.jcarousel.min.js", "~/Scripts/wysihtml5-0.3.0.min.js",
                "~/Scripts/uncompressed/bootstrap-wysihtml5.js","~/Scripts/app/app_form.js", "~/Scripts/chosen.jquery.min.js"
                ));

            //add jquery globalization library
          //  bundles.Add(new ScriptBundle("~/bundles/globalization").IncludeDirectory("~/Scripts/globalize-culture", "*.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new Bundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new Bundle("~/bundles/home").Include(
                      "~/Scripts/app/home.js"));
            

            //custom js for alerts application
            bundles.Add(new Bundle("~/bundles/alerts").Include("~/Scripts/app/alerts.js"));

            bundles.Add(new Bundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css", "~/Content/pace.css", "~/Content/slider.css", "~/Content/app.min.css", "~/Content/jquery.dataTables_themeroller.css",
                      "~/Content/font-awesome.min.css", "~/Content/morris.css", "~/Content/colorbox.css", "~/Content/summernote.css"));
            //scripts and css for the public interface
            bundles.Add(new Bundle("~/bundles/public").Include(
                      "~/Scripts/app/public.js", "~/Scripts/app/GPX.js", "~/Scripts/Leaflet.GoogleMutant.js", "~/Scripts/app/easy-button.min.js"));

            //custom bundle for theme
            bundles.Add(new Bundle("~/bundles/publicjs").Include("~/Scripts/pace.min.js", "~/Scripts/jquery-{version}.js",
                 "~/Scripts/jquery.dataTables.min.js", "~/Scripts/jquery.slimscroll.min.js", "~/Scripts/jquery.popupoverlay.min.js",
                 "~/Scripts/app/app.js"));

            bundles.Add(new Bundle("~/Content/pubcss").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css", "~/Content/pace.css",  "~/Content/app-public.css", "~/Content/jquery.dataTables_themeroller.css",
                      "~/Content/font-awesome.min.css", "~/Content/MarkerCluster.css", "~/Content/MarkerCluster.Default.css"));


            bundles.Add(new Bundle("~/Content/css/form").Include(
                 "~/Content/colorbox/colorbox.min.css", "~/Content/dropzone/dropzone.css", "~/Content/bootstrap-wysihtml5.css", /* "~/Content/bootstrap-timepicker.css", "~/Content/datepicker.css",*/  "~/Content/jquery.datetimepicker.css"
                ));

            BundleTable.EnableOptimizations = true;
            bundles.UseCdn = true;
        }
    }
}
