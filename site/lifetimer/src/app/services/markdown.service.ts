import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MarkdownService {

  constructor(private http: HttpClient) { }

  /**
   * Loads markdown content from a specified URL
   * @param url The URL to load markdown content from
   * @returns Observable<string> The markdown content as a string
   */
  loadMarkdown(url: string): Observable<string> {
    return this.http.get(url, { responseType: 'text' })
      .pipe(
        catchError(error => {
          console.error('Error loading markdown:', error);
          return of('# Error\nUnable to load documentation.');
        })
      );
  }

  /**
   * Loads the default documentation file
   * @returns Observable<string> The documentation markdown content
   */
  loadDocumentation(): Observable<string> {
    return this.loadMarkdown('/docs/document.md');
  }
}