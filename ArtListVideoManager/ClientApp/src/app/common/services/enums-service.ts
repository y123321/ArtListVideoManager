import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { baseApiUrl } from "../consts";
@Injectable()
export default class EnumsService{
  private controllerUrl: string
  constructor(private http: HttpClient) {//, @Inject('BASE_URL') private baseUrl: string) {
    this.controllerUrl = baseApiUrl + "enums/";
  }
  getVideoFormats(): Observable<string[]> {
    return this.http.get<string[]>(this.controllerUrl + 'videoFormats');
  }
}
