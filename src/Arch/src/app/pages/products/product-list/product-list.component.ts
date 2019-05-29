import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Observable } from 'rxjs';
import { FormBuilder, FormGroup } from '@angular/forms';
import { startWith, map } from 'rxjs/operators';
import { ProductService, ProductDw } from '../product.service';
export interface StateGroup {
  letter: string;
  names: string[];
}


export const _filter = (opt: string[], value: string): string[] => {
  const filterValue = value.toLowerCase();

  return opt.filter(item => item.toLowerCase().indexOf(filterValue) === 0);
};

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  filtersLoaded: Promise<boolean>;
  [x: string]: any;

  products: ProductDw[];
  stateForm: FormGroup = this.fb.group({
    stateGroup: '',
  });

  
  
  stateGroupOptions: Observable<StateGroup[]>;

  constructor(
    private fb: FormBuilder, private productService: ProductService) {
    }
    stateGroups: StateGroup[];
  ngOnInit() {
    this.stateGroupOptions = this.stateForm.get('stateGroup')!.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filterGroup(value))
      );
      this.getProducts();

      this.filtersLoaded = Promise.resolve(true);
  }

  actualize() {

    this.stateGroupOptions = this.stateForm.get('stateGroup')!.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filterGroup(value))
      );
  }

  changed() {
    console.log("changed")
  }

  getProducts() {
    
    this.productService.getAll().subscribe(
      products => {
        this.stateGroups = this.convertProducts(products);
      }
    );
  }

  convertProducts(dataIn: ProductDw[]): StateGroup[] {
    let data = dataIn.reduce((r, e) => {
      let letter = e.name[0];
      if (!r[letter]) { r[letter] = {letter, names: [e.name]} } else { r[letter].names.push(e.name); }
      
      return r;
    }, {})

    let result: StateGroup[] = Object.values(data)
    return result;
  }

  private _filterGroup(value: string): StateGroup[] {
    if (value) {
      return this.stateGroups
        .map(group => ({letter: group.letter, names: _filter(group.names, value)}))
        .filter(group => group.names.length > 0);
    }
    return this.stateGroups;
  }

}
