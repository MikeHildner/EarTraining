import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { APP_BASE_HREF } from '@angular/common';
import { MatSliderModule } from '@angular/material/slider';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';

//import { MatToolbarModule, MatIconModule, MatSidenavModule, MatListModule, MatButtonModule } from '@angular/material/sidenav';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatRippleModule } from '@angular/material/core';
import { MatListModule } from "@angular/material/list";
import { MatMenuModule } from '@angular/material/menu';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { MatCardModule } from '@angular/material/card';
import { L1C1VocalDrillsComponent } from './level1/chapter1/l1-c1-vocal-drills/l1-c1-vocal-drills.component';
import { DoComponent } from './do/do.component';
import { NgxAudioPlayerModule } from 'ngx-audio-player';
import { FlexLayoutModule } from '@angular/flex-layout';
import { DashboardComponent } from './dashboard/dashboard.component';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatButtonModule } from '@angular/material/button';
import { LayoutModule } from '@angular/cdk/layout';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AboutComponent,
    L1C1VocalDrillsComponent,
    DoComponent,
    DashboardComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    MatSliderModule,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatRippleModule,
    MatListModule,
    MatMenuModule,
    MatCardModule,
    NgxAudioPlayerModule,
    FlexLayoutModule,
    MatGridListModule,
    MatButtonModule,
    LayoutModule,
  ],
  exports: [
    
  ],
  //providers: [],
  providers: [{ provide: APP_BASE_HREF, useValue: '/angulareartraining' }],
  bootstrap: [AppComponent]
})
export class AppModule {
  
}
