namespace FireEscape.Views.BaseViews
{
    public abstract class BaseUserAccountPage<T> : BaseEditPage<BaseEditViewModel<UserAccount>, UserAccount>
    {
        protected BaseUserAccountPage(BaseEditViewModel<UserAccount> viewModel) : base(viewModel)
        {
        }
    }
}
