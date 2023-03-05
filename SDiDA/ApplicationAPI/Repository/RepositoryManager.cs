using System;

namespace ApplicationAPI.Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IUsersProfilesRepository> _userProfilesRepository;
        private readonly Lazy<IPostsRepository> _postsRepository;
        private readonly Lazy<ICommentsRepository> _commentsRepository;
        private readonly Lazy<IUsersAccountsRepository> _userAccountsRepository;
        public RepositoryManager(IMongoDbSettings settings)
        {
            _userProfilesRepository = new Lazy<IUsersProfilesRepository>(() => new UserProfilessRepository(settings));
            _postsRepository = new Lazy<IPostsRepository>(() => new PostsRepository(settings));
            _commentsRepository = new Lazy<ICommentsRepository>(() => new CommentsRepository(settings));
            _userAccountsRepository = new Lazy<IUsersAccountsRepository>(() => new UserAccountsRepository(settings));
        }

        public IUsersProfilesRepository UserProfilesRepository => _userProfilesRepository.Value;
        public IPostsRepository PostsRepository => _postsRepository.Value;
        public ICommentsRepository CommentsRepository => _commentsRepository.Value;
        public IUsersAccountsRepository UserAccountsRepository => _userAccountsRepository.Value;
    }
}
