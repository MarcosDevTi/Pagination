import {Component, OnInit} from '@angular/core';
import {FormControl, FormBuilder} from '@angular/forms';
import {Observable, BehaviorSubject} from 'rxjs';
import {map, startWith, scan} from 'rxjs/operators';

export interface User {
  id: string;
  name: string;
}

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  
  total = 300;
  data = [
    'Alabama', 'Alaska', 'Arizona', 'Arkansas', 'California', 'Colorado', 'Connecticut', 'Delaware', 'Florida', 'Georgia', 'Hawaii', 'Idaho', 
    'Illinois', 'Indiana', 'Iowa', 'Maine', 'Maryland', 'Massachusetts', 'Michigan',
    'Minnesota', 'Mississippi', 'Missouri', 'Montana', 'Nebraska', 'Nevada', 'New Hampshire', 'New Jersey',
    'New Mexico', 'New York', 'North Carolina', 'North Dakota', 'Washington', 'West Virginia', 'Wisconsin', 'Wyoming'
  ]
  limit = 10;
  offset = 0;
  options2 = new BehaviorSubject<User[]>([]);
  options$: Observable<User[]>;



  myControl = new FormControl();
  
  formProd = this.fb.group({});
  options: User[] = [
    {id: '1234', name: 'Mary'},
    {id: '4321', name: 'Shelley'},
    {id: '4242', name: 'Igor'},
    {id: '1234', name: 'Mary'},
    {id: '4321', name: 'Shelley'},
    {id: '4242', name: 'Igor'},
    {id: '1234', name: 'Mary'},
    {id: '4321', name: 'Shelley'},
    {id: '4242', name: 'Igor'},
    {id: '1234', name: 'Mary'},
    {id: '4321', name: 'Shelley'},
    {id: '4242', name: 'Igor'},
    {id: '1234', name: 'Mary'},
    {id: '4321', name: 'Shelley'},
    {id: '4242', name: 'Igor'},
    {id: '1234', name: 'Mary'},
    {id: '4321', name: 'Shelley'},
    {id: '4242', name: 'Igor'},
    {id: '1234', name: 'Mary'},
    {id: '4321', name: 'Shelley'},
    {id: '4242', name: 'Igor'},
    {id: '1234', name: 'Mary'},
    {id: '4321', name: 'Shelley'},
    {id: '4242', name: 'Igor'},
  ];
  filteredOptions: Observable<User[]>;
constructor(private fb: FormBuilder) {
  this.options$ = this.options2.asObservable().pipe(
    scan((acc, curr) => {
      return [...acc, ...curr];
    }, [])
  );
}
  ngOnInit() {
    this.getNextBatch();
    this.filteredOptions = this.myControl.valueChanges
      .pipe(
        startWith(''),
        map(value => typeof value === 'string' ? value : value.name),
        map(name => name ? this._filter(name) : this.options.slice())
      );
  }

  getNextBatch() {
    console.log("mudou")
    const result = this.options.slice(this.offset, this.offset + this.limit);
    this.options2.next(result);
    this.offset += this.limit;
  }

  displayFn(user?: User): string | undefined {
    return user ? user.name : undefined;
  }

  private _filter(name: string): User[] {
    const filterValue = name.toLowerCase();

    return this.options.filter(option => option.name.toLowerCase().indexOf(filterValue) === 0);
  }

  onSubmit(){}

  selecionar(id: string) {
    console.log(id)
  }

}
