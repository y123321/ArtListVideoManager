import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { UploadFormComponent } from './upload-form/upload-form.component';
import { VideosListComponent } from './video/videos-list.component';
import { VideoItemComponent } from './video/video-item.component';
import VideoListResolver from './common/services/video-list-resolver.service';
import VideoService from './common/services/video-service';
import EnumsService from './common/services/enums-service';
;

@NgModule({
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: VideosListComponent, pathMatch: 'full', resolve: { videos: VideoListResolver } },
      { path: 'upload', component: UploadFormComponent },
    ]),
    NgbModule
  ],
  declarations: [
    AppComponent,
    NavMenuComponent,
    UploadFormComponent,
    VideosListComponent,
    VideoItemComponent,
  ],
  providers: [VideoService, VideoListResolver, EnumsService],
  bootstrap: [AppComponent]
})
export class AppModule { }
