import { Component, OnInit } from '@angular/core';
import { NavItem } from './NavItem';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {

  constructor() { }

  menuData: NavItem[] = [
    {nom: 'Accueil', url: '', navItems: []},
    {nom: 'Customers', url: '/customers',
      navItems: [
                  {nom: 'Customers', url: '/customers', navItems: []},
                  {nom: 'New Customer', url: '/customers/new', navItems: []},
                ]
    },
    {nom: 'Déclaration', url: '',
      navItems: [
                  {nom: 'SubMenu 1', url: '#', navItems: []},
                  {nom: 'SubMenu 2', url: '#', navItems:
                    [
                      {nom: 'SubSubMenu 1', url: '#', navItems: []},
                      {nom: 'SubSubMenu 2', url: '#', navItems:
                      [
                        {nom: 'SSubSubMenu 1', url: '#', navItems: []},
                        {nom: 'SSubSubMenu 2', url: '#', navItems: []},
                      ]
                    },
                    ]
                },
                ]
    },
    {nom: 'Products', url: '',
    navItems: [
                {nom: 'Products', url: '/products', navItems: []},
                {nom: 'New Product', url: '/products/new', navItems: []},
              ]
    },
    {nom: 'Pilotage', url: '', navItems: []},
    {nom: 'Aide', url: '', navItems: []}
    ];
    
  ngOnInit() {
  }

}
