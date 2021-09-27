import { NavMenu } from "../models/navmenu.model";

export let menu: NavMenu[] = [
    {
        displayName: 'Home',
        closedName: 'Home',
        iconName: 'home',
        routerLink: '/home',
        roles: ['SuperAdmin', 'ContentAdmin']
    },
    {
        displayName: 'Content Providers',
        closedName: 'Providers',
        iconName: 'play_lesson',
        routerLink: 'admin/content-providers',
        roles: ['SuperAdmin', 'ContentAdmin']
    },
    {
        displayName: 'SAS Key',
        closedName: 'SAS Key',
        iconName: 'vpn_key',
        routerLink: 'admin/sas-key',
        roles: ['SuperAdmin', 'ContentAdmin']
    },
    {
        displayName: 'Subscriptions',
        closedName: 'Subscriptions',
        iconName: 'subscriptions',
        routerLink: 'admin/subscriptions',
        roles: ['SuperAdmin']
    },
    {
        displayName: 'Notifications',
        closedName: 'Notifications',
        iconName: 'notifications',
        routerLink: 'admin/notifications',
        roles: ['SuperAdmin']
    },
    {
        displayName: 'Unprocessed',
        closedName: 'Unprocessed',
        iconName: 'movie',
        routerLink: 'admin/unprocessed-content',
        roles: ['SuperAdmin']
    },
    {
        displayName: 'Processed',
        closedName: 'Processed',
        iconName: 'local_movies',
        routerLink: 'admin/processed-content',
        roles: ['SuperAdmin']
    },
    {
        displayName: 'Broadcast',
        closedName: 'Broadcast',
        iconName: 'live_tv',
        routerLink: 'admin/broadcast-content',
        roles: ['SuperAdmin']
    },
    {
        displayName: 'Incentives',
        closedName: 'Incentives',
        iconName: 'emoji_events',
        routerLink: 'admin/incentive-management',
        roles: ['SuperAdmin']
    },
    {
        displayName: 'Devices',
        closedName: 'Devices',
        iconName: 'router',
        routerLink: 'admin/devices',
        roles: ['SuperAdmin']
    },
    {
        displayName: 'Dashboard',
        closedName: 'Dashboard',
        iconName: 'leaderboard',
        routerLink: '/retailer/dashboard',
        roles: ['Retailer']
    },
    {
        displayName: 'Orders',
        closedName: 'Orders',
        iconName: 'shopping_bag',
        routerLink: '/retailer/orders',
        roles: ['Retailer']
    }
]