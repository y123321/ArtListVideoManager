import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import VideoItemModel from "../models/video-item.model";
import { baseApiUrl } from "../consts";
@Injectable()
export default class VideoService {
  private controllerUrl: string;
  constructor(private http: HttpClient) {//, @Inject('BASE_URL') private baseUrl: string) {
    this.controllerUrl = baseApiUrl + "videos/"
  }
  getVideos(): Observable<VideoItemModel[]> {
    return this.http.get<VideoItemModel[]>(this.controllerUrl);
  }
  uploadVideo(data, file: File) {//: Observable<any>{
    const formData: FormData = new FormData();
    for (const prop in data)
      formData.append(prop, data[prop]);
    formData.append("file", file);
    return this.http.post(this.controllerUrl, formData, { reportProgress: true, observe: "events" });
      
  }
}
