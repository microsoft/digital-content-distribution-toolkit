import { environment } from "src/environments/environment";
import { NavMenu } from "../models/navmenu.model";

export let menu: NavMenu[] = [
    {
        displayName: 'Home',
        closedName: 'Home',
        iconName: 'home',
        routerLink: '/home',
        roles: ['SuperAdmin', 'ContentAdmin'],
        featureName: environment.featureName.Home
    },
    {
        displayName: 'Content Providers',
        closedName: 'Providers',
        iconName: 'play_lesson',
        routerLink: 'admin/content-providers',
        roles: ['SuperAdmin', 'ContentAdmin'],
        featureName: environment.featureName.ContentProviders
    },
    {
        displayName: 'SAS Key',
        closedName: 'SAS Key',
        iconName: 'vpn_key',
        routerLink: 'admin/sas-key',
        roles: ['SuperAdmin', 'ContentAdmin'],
        featureName: environment.featureName.SASKey
    },
    {
        displayName: 'Unprocessed',
        closedName: 'Unprocessed',
        iconName: 'movie',
        routerLink: 'admin/unprocessed-content',
        roles: ['SuperAdmin'],
        featureName: environment.featureName.Unprocessed
    },
    {
        displayName: 'Processed',
        closedName: 'Processed',
        iconName: 'local_movies',
        routerLink: 'admin/processed-content',
        roles: ['SuperAdmin'],
        featureName: environment.featureName.Processed
    },
    {
        displayName: 'Broadcast',
        closedName: 'Broadcast',
        iconName: 'live_tv',
        routerLink: 'admin/broadcast-content',
        roles: ['SuperAdmin'],
        featureName: environment.featureName.Broadcast
    },
    {
        displayName: 'Subscriptions',
        closedName: 'Subscriptions',
        iconName: 'subscriptions',
        routerLink: 'admin/subscriptions',
        roles: ['SuperAdmin'],
        featureName: environment.featureName.Subscriptions
    },
    {
        displayName: 'Incentives',
        closedName: 'Incentives',
        iconName: 'emoji_events',
        routerLink: 'admin/incentive-management',
        roles: ['SuperAdmin'],
        featureName: environment.featureName.Incentives
    },
    {
        displayName: 'Devices',
        closedName: 'Devices',
        iconName: 'router',
        routerLink: 'admin/devices',
        roles: ['SuperAdmin', 'HubDeviceManagement'],
        featureName: environment.featureName.Devices
    },
    {
        displayName: 'Notifications',
        closedName: 'Notifications',
        iconName: 'notifications',
        routerLink: 'admin/notifications',
        roles: ['SuperAdmin'],
        featureName: environment.featureName.Notifications
    },
    {
        displayName: 'Export',
        closedName: 'Export',
        iconName: 'send_to_mobile',
        routerLink: 'admin/export',
        roles: ['SuperAdmin'],
        featureName: environment.featureName.Export
    },
    {
        displayName: 'Delete',
        closedName: 'Delete',
        iconName: 'delete',
        routerLink: 'admin/delete',
        roles: ['SuperAdmin'],
        featureName: environment.featureName.Delete
    },
    
    {
        displayName: 'Retailer Dashboard',
        closedName: 'Retailer',
        iconName: 'leaderboard',
        routerLink: 'admin/retailer-dashboard',
        roles: ['SuperAdmin'],
        featureName: environment.featureName.RetailerDashboard
    },
    {
        displayName: 'Dashboard',
        closedName: 'Dashboard',
        iconName: 'leaderboard',
        routerLink: '/retailer/dashboard',
        roles: ['Retailer'],
        featureName: environment.featureName.Dashboard
    },
    {
        displayName: 'Orders',
        closedName: 'Orders',
        iconName: 'shopping_bag',
        routerLink: '/retailer/orders',
        roles: ['Retailer'],
        featureName: environment.featureName.Order
    }
]