namespace ISynergy.Framework.Mvvm.Extensions
{
    /// <summary>
    /// ViewModel extensions.
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// In case of an generic viewmodel, this function returns the base name from IViewModel. 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static string GetViewModelName(this IViewModel viewModel) =>
            viewModel.GetType().GetViewModelName();

        /// <summary>
        /// In case of an generic viewmodel, this function returns the base name from Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetViewModelName(this Type type)
        {
            var result = type.Name;

            if (type.IsGenericType)
            {
                result = type.GetGenericTypeDefinition().Name.Remove(type.GetGenericTypeDefinition().Name.IndexOf('`'));
            }

            return result;
        }

        /// <summary>
        /// In case of an generic viewmodel, this function returns the base name from IViewModel. 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static string GetViewModelFullName(this IViewModel viewModel) =>
            viewModel.GetType().GetViewModelFullName();

        /// <summary>
        /// In case of an generic viewmodel, this function returns the base name from Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetViewModelFullName(this Type type)
        {
            var result = type.FullName;

            if (type.IsGenericType)
            {
                result = type.GetGenericTypeDefinition().FullName.Remove(type.GetGenericTypeDefinition().FullName.IndexOf('`'));
            }

            return result;
        }
    }
}
