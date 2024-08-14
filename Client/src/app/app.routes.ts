import { Routes, CanActivateChildFn } from '@angular/router';
import { TabsPage } from './screens/tabs/tabs.page';
import { authenticateGuard, authenticateGuardForChild } from './services/authentication/can-activate-jwt.guard';
import { AppRoute } from './screens/app-routes.enum';
import { RegisterComponent } from './screens/register/register.component';

export const routes: Routes = [
  {
    path: "",
    redirectTo: AppRoute.Login,
    pathMatch: "full",
  },
  {
    path: AppRoute.Login,
    component: RegisterComponent,
    pathMatch: "full",
    canActivate: [authenticateGuard],
  },
  {
    path: AppRoute.Home,
    component: TabsPage,
    canActivate: [authenticateGuard],
    canActivateChild: [authenticateGuardForChild],
    pathMatch: "prefix",
    children: [
      {
        path: "",
        redirectTo: AppRoute.Explore,
        pathMatch: "full",
      },
      {
        path: AppRoute.Explore,
        loadComponent: () =>
          import('./screens/tabs/explore/explore.component').then((m) => m.ExploreComponent),
      },
      {
        path: AppRoute.Property_List,
        loadComponent: () =>
          import('./screens/tabs/properties/properties.component').then((m) => m.PropertiesComponent),
      },
      {
        path: 'tab3',
        loadComponent: () =>
          import('./screens/tabs/tab3/tab3.page').then((m) => m.Tab3Page),
      }
    ]
  },
  {
    path: "**",
    redirectTo: AppRoute.Home,
  }
];
