import { Component, signal } from '@angular/core';

export interface Product {
  title: string;
  url: string;
}

@Component({
  selector: 'app-footer',
  imports: [],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {
  products = signal<Array<Product>>([
    {
      title: 'DeskNote',
      url: 'https://desknote.app',
    },
    {
      title: 'DeskWeb',
      url: 'https://deskweb.app',
    },
    {
      title: 'HMITMC',
      url: 'https://howmuchisthismeetingcosting.com',
    },
    {
      title: 'LifeTimer',
      url: 'https://parseclifetimer.app',
    },
    {
      title: 'DeskQuote',
      url: 'https://deskquote.app',
    },
  ]);
}
