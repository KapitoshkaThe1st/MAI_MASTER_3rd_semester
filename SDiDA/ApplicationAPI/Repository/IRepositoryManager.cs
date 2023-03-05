namespace ApplicationAPI.Repository
{
    public interface IRepositoryManager
    {
        public IUsersProfilesRepository UserProfilesRepository { get; }
        public IPostsRepository PostsRepository { get; }
        public ICommentsRepository CommentsRepository { get; }
        public IUsersAccountsRepository UserAccountsRepository { get; }
    }
}
