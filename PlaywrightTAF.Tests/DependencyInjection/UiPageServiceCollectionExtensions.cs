using Microsoft.Extensions.DependencyInjection;
using PlaywrightTAF.UI.Components;
using PlaywrightTAF.UI.Pages;
using PlaywrightTAF.UI.Pages.AdminPages;
using PlaywrightTAF.UI.Pages.AdminPages.Base;
using PlaywrightTAF.UI.Pages.Base;
using PlaywrightTAF.UI.Pages.NewsFeedPages;
using PlaywrightTAF.UI.Pages.NewsFeedPages.Base;

namespace PlaywrightTAF.Tests.DependencyInjection;

internal static class UiPageServiceCollectionExtensions
{
    public static IServiceCollection AddUiPageObjects(this IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<MainPage>();
        services.AddTransient<DashboardPage>();
        services.AddTransient<Dropdown>();
        services.AddTransient<ToastMessage>();

        services.AddTransient<BasePageAdmin>();
        services.AddTransient<AdminCorporateBrandingPage>();

        services.AddTransient<NewsFeedBasePage>();
        services.AddTransient<MostLikedPostsPage>();

        services.AddTransient<PimConfigurationBasePage>();
        services.AddTransient<CustomFieldsPage>();
        services.AddTransient<DataImportPage>();

        services.AddTransient<AddUserPage>();
        services.AddTransient<DeleteUserPage>();
        services.AddTransient<EditUserPage>();
        services.AddTransient<PersonalDetailsPage>();

        return services;
    }
}
