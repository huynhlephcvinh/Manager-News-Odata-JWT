using BusinessObject;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace NewsManagementSystem_HuynhLePhucVinh
{
    public class ModelBuilder
    {
        public static IEdmModel GetEDMModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();

            // Expose Entity Sets
            odataBuilder.EntitySet<SystemAccount>("SystemAccounts").
            EntityType.HasKey(sa => sa.AccountId); // Explicitly defining the key
            odataBuilder.EntitySet<Category>("Categories");
            //   .EntityType.HasRequired(s => s.ParentCategory);
            odataBuilder.EntitySet<NewsArticle>("NewsArticles")
                .EntityType.HasKey(sa => sa.NewsArticleId)
                ;
            odataBuilder.EntitySet<Tag>("Tags").EntityType.HasKey(sa => sa.TagId);

            // The relationships between entities should already be defined in your entity models
            // No need to explicitly define HasMany/WithMany here for OData

            // Define the final EDM model
            return odataBuilder.GetEdmModel();
        }
    }
}
