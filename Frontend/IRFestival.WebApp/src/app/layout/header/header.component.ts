import { Component, OnInit } from '@angular/core';
import { AppSettingsApiService } from 'src/app/api-services/appsettings-api.service';
import { environment } from 'src/environments/environment';
import { MsalService, MsalBroadcastService } from '@azure/msal-angular';

import { InteractionStatus } from '@azure/msal-browser';
import { filter } from 'rxjs';
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit {
  festivalName = environment.festivalName;
  loginDisplay: boolean;

  constructor(private appSettingsApiService: AppSettingsApiService, private msalService:MsalService, private broadcastService:MsalBroadcastService) {}

  ngOnInit(): void {
    this.appSettingsApiService
      .getSettings()
      .subscribe(
        (appsettings) =>
          (this.festivalName =
            appsettings.festivalName ?? environment.festivalName)
      );

      this.broadcastService.inProgress$
            .pipe(
              filter((status:InteractionStatus)=>status===InteractionStatus.None)            
            )
            .subscribe(()=>{
              this.setLoginDisplay();
            })
  }


  login(){
    this.msalService.loginRedirect();
  }
  logout(){
    this.msalService.logoutRedirect({
      postLogoutRedirectUri:environment.redirectUrl
    });
  }

  setLoginDisplay(){
    this.loginDisplay=this.msalService.instance.getAllAccounts().length>0;
  }

}


