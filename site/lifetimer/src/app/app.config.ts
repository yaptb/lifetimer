import {
  ApplicationConfig,
  provideZoneChangeDetection,
  SecurityContext,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideMarkdown, MARKED_OPTIONS } from 'ngx-markdown';
import { gfmHeadingId } from 'marked-gfm-heading-id';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideMarkdown({
      sanitize: SecurityContext.NONE,
      markedOptions: {
        provide: MARKED_OPTIONS,
        useValue: {
          gfm: true, // Enables GitHub Flavored Markdown
          breaks: true, // Enables line breaks on newlines
        },
      },
      markedExtensions: [gfmHeadingId()],
    }),
  ],
};
