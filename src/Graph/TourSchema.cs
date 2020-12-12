using Microsoft.VisualStudio.GraphModel;
using Microsoft.VisualStudio.GraphModel.Schemas;

namespace CodeTourVS
{
    internal class TourSchema
    {
        static TourSchema()
        {
            TourToStepLink.BasedOnCategory = CodeLinkCategories.Contains;
        }

        public static GraphSchema Schema = new GraphSchema("TourSchema");
        public static GraphCategory Tour = Schema.Categories.AddNewCategory("TourFile");
        public static GraphCategory TourToStepLink = Schema.Categories.AddNewCategory("TourToStepLink");
        public static GraphCategory Step = Schema.Categories.AddNewCategory("TourStep");
        public static GraphNodeIdName StepValueName = GraphNodeIdName.Get("TourStepValueName", null, typeof(string), true);
    }
}
