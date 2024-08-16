import { Routes } from '@angular/router';
import { AppRoute } from './screens/app-routes.enum';
import { RegisterComponent } from './screens/register/register.component';
import { TabsPage } from './screens/tabs/tabs.page';
import { authenticateGuard, authenticateGuardForChild } from './services/authentication/can-activate-jwt.guard';

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
      }
    ]
  },
  {
    path: "**",
    redirectTo: AppRoute.Home,
  }
];
