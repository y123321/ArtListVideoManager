import { Component, Input, OnInit } from "@angular/core";
import VideoItemModel from "../common/models/video-item.model";




@Component({
  selector: "video-item",
  templateUrl: "./video-item.component.html",
})
export class VideoItemComponent implements OnInit {
  @Input() item: VideoItemModel;
  thumnailIndex = 0;
  ngOnInit(): void {
    if (this.item.thumbnailLinks)
      setInterval(() => {
        this.thumnailIndex = (this.thumnailIndex + 1) % this.item.thumbnailLinks.length
      },3000)
  }

}

