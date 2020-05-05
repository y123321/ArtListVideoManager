import { Component, OnInit } from "@angular/core";
import VideoService from "../common/services/video-service";
import { ActivatedRoute } from "@angular/router";
import EnumsService from "../common/services/enums-service";
import { HttpEventType } from "@angular/common/http";
@Component({
  templateUrl: "./upload-form.component.html",
})
export class UploadFormComponent implements OnInit {
  name;
  videoFormat;
  file: File;
  uploadProress: number;
  isUploading: boolean;
  private videoFormats: string[]

  constructor(private videoService: VideoService, private route: ActivatedRoute, private enumsService: EnumsService) {
  }
  ngOnInit() {
    this.isUploading = false;
    this.enumsService.getVideoFormats().subscribe(formats => {
      this.videoFormats = formats
      this.videoFormat = this.videoFormats[0];
    });
  }
  selectFile($event) {
    this.file = $event.target.files[0];
  }

  upload(data) {
    this.isUploading = true;
    this.uploadProress = 0;
    this.videoService.uploadVideo(data, this.file)
      .subscribe(event => {
        if (event.type === HttpEventType.DownloadProgress) {
          this.uploadProress = Math.round(100 * event.loaded / event.total);
          console.log("download progress");
        }
        if (event.type === HttpEventType.Response) {
          console.log("donwload completed");
          this.isUploading = false;
        }
      }, err => {
          console.log(err);
          this.isUploading = false;
      })
  }
}
