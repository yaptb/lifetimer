import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { StoreComponent } from './pages/store/store.component';
import { DocsComponent } from './pages/docs/docs.component';
import { HelpComponent } from './pages/help/help.component';
import { DocumentContentComponent } from './pages/docs/components/document-content.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'store', component: StoreComponent },
  { 
    path: 'docs', 
    component: DocsComponent,
    children: [
      { path: '', component: DocumentContentComponent },
      { path: 'document', component: DocumentContentComponent }
    ]
  },
  { path: 'help', component: HelpComponent },
  { path: '**', redirectTo: '/home' }
];
