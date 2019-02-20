import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MaterialModule } from './../../material/material.module';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { SolutionComponent } from './solution/solution.component';
import { BusinessFlowsComponent } from './businessflows/businessflows.component';
import { ServicesGridComponent } from './servicesGrid/servicesGrid.component';
import { RunSetComponent } from './runset/runset.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FlexLayoutModule } from '@angular/flex-layout';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    SolutionComponent,
    BusinessFlowsComponent,
    ServicesGridComponent,
    RunSetComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NgbModule,
    MaterialModule,
    BrowserAnimationsModule,
    FlexLayoutModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'solution', component: SolutionComponent },
      { path: 'businessflows', component: BusinessFlowsComponent },
      { path: 'servicesGrid', component: ServicesGridComponent },
      { path: 'runset', component: RunSetComponent },
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
