import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PicturesApiService {
  private baseUrl = environment.apiBaseUrl + 'pictures';

  constructor(private httpClient: HttpClient) { }

  getAllUrls(): Observable<string[]> {
    const headers = new HttpHeaders().set("Ocp-Apim-Subscription-Key", "87af1e6f5c8649c59431acd66a26949b");
    return this.httpClient.get<string[]>(`${this.baseUrl}`, {'headers':headers});
  }

  upload(file: File): Observable<never> {
    const headers = new HttpHeaders().set("Ocp-Apim-Subscription-Key", "87af1e6f5c8649c59431acd66a26949b");
    const data = new FormData();
    data.set('file', file);

    return this.httpClient.post<never>(`${this.baseUrl}/upload`, data, {'headers':headers});
  }
}
