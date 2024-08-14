import { Routes } from '@angular/router';
import { AppRoute } from './pages/app-routes.enum';
import { RegisterComponent } from './pages/register/register.component';
import { TabsPage } from './tabs/tabs.page';
import { authenticateGuard } from './services/authentication/can-activate-jwt.service';

export const routes: Routes = [
  {
    path: "",
    redirectTo: AppRoute.Register,
    pathMatch: "full",
  },
  {
    path: AppRoute.Register,
    component: RegisterComponent,
    pathMatch: "full",
  },
  {
    path: AppRoute.Tabs,
    component: TabsPage,
    canActivate: [authenticateGuard],
    children: [
      {
        path: 'tab1',
        loadComponent: () =>
          import('./tab1/tab1.page').then((m) => m.Tab1Page),
      },
      {
        path: AppRoute.Property_List,
        loadComponent: () =>
          import('./tab2/tab2.page').then((m) => m.Tab2Page),
      },
      {
        path: 'tab3',
        loadComponent: () =>
          import('./tab3/tab3.page').then((m) => m.Tab3Page),
      }
    ]
  },
  {
    path: "**",
    redirectTo: AppRoute.Register,
  }
];
