using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System.Linq;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Statistics
    /// </summary>
    public class StatsRepository : IStatsRepository
    {
        /// <summary>
        /// Get stats info
        /// </summary>
        /// <returns>Stats counters</returns>
        public Stats Get()
        {
            
            var stats = new Stats();

            var postList = Post.ApplicablePosts.Where(p => p.IsVisible).ToList();

            if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
                postList = postList.Where(p => p.Author.ToLower() == Security.CurrentUser.Identity.Name.ToLower()).ToList();
            if (Security.IsAdministrator || Security.IsChief)
            {

                stats.PublishedPostsCount = postList.Where(p => p.IsPublished == true).Count();
                stats.DraftPostsCount = postList.Where(p => p.IsPublished == false).Count();

                stats.PublishedPagesCount = Page.Pages.Where(p => p.IsPublished == true && p.IsDeleted == false).Count();
                stats.DraftPagesCount = Page.Pages.Where(p => p.IsPublished == false && p.IsDeleted == false).Count();
                stats.UsersCount = 3;
            }
            else
            {
                stats.PublishedPostsCount = postList.Where(p => p.IsPublished == true && p.Author.ToLower() == Security.CurrentUser.Identity.Name.ToLower()).Count();
                stats.DraftPostsCount = postList.Where(p => p.IsPublished == false && p.Author.ToLower() == Security.CurrentUser.Identity.Name.ToLower()).Count();

                stats.PublishedPagesCount = 0;
                stats.DraftPagesCount = 0;
                stats.UsersCount = 0;
            }
            CountComments(stats);

            stats.CategoriesCount = Category.Categories.Count;
            stats.TagsCount = 2;
            

            return stats;
        }

        void CountComments(Stats stats)
        {
            
            foreach (var post in Post.Posts)
            {
                if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.EditOtherUsersPosts))
                    if (post.Author.ToLower() != Security.CurrentUser.Identity.Name.ToLower())
                        continue;

                stats.PublishedCommentsCount += post.Comments.Where(c => c.IsPublished == true && c.IsDeleted == false).Count();
                stats.UnapprovedCommentsCount += post.Comments.Where(c => c.IsPublished == false && c.IsSpam == false && c.IsDeleted == false).Count();
                stats.SpamCommentsCount += post.Comments.Where(c => c.IsPublished == false && c.IsSpam == true && c.IsDeleted == false).Count();
            }
        }
    }
}
