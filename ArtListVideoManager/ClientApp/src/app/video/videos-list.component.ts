import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import VideoItemModel from '../common/models/video-item.model';
@Component({
  selector:"video-list",
  templateUrl:"./videos-list.component.html"
})
export class VideosListComponent implements OnInit {
  videos: VideoItemModel[];
  constructor(private route: ActivatedRoute) {
  }
  ngOnInit() {
    this.videos = this.route.snapshot.data["videos"];
  }
}
