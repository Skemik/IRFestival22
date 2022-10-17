import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './layout/header/header.component';
import { FooterComponent } from './layout/footer/footer.component';
import { LayoutModule } from './layout/layout.module';
import { HttpClientModule } from '@angular/common/http';
import { MsalModule,MsalRedirectComponent } from '@azure/msal-angular';
import { PublicClientApplication } from '@azure/msal-browser';
import { environment } from 'src/environments/environment';
@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,

    HttpClientModule,

    LayoutModule,
  

  MsalModule.forRoot( new PublicClientApplication({

    auth: {

      clientId: '76afe3ac-2cd2-4477-b21d-e7b0c975652f',

      authority: 'https://login.microsoftonline.com/4c042412-4355-4264-a010-99400d55bf73',

      redirectUri: environment.redirectUrl

    },

    cache: {

      cacheLocation: 'localStorage'

    }

  }), null, null)

],

providers: [],

bootstrap: [AppComponent, MsalRedirectComponent]
})
export class AppModule { }
