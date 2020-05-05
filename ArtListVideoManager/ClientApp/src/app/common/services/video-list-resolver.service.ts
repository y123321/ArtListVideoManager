import { Injectable } from "@angular/core";
import { Resolve } from "@angular/router"
import VideoService from "./video-service";
import { map } from "rxjs/operators"
import VideoItemModel from "../models/video-item.model";
@Injectable()
export default class VideoListResolver implements Resolve<VideoItemModel[]> {
  constructor(private videoService: VideoService) {

  }
  resolve() {
    return this.videoService.getVideos().pipe(map(videos => videos));
  }
}
