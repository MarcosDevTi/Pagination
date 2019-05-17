import { Component, OnInit, Input } from '@angular/core';
import { CustomerService } from '../customer.service';



@Component({
  selector: 'app-customer-history',
  templateUrl: './customer-history.component.html',
  styleUrls: ['./customer-history.component.css']
})
export class CustomerHistoryComponent implements OnInit {
  @Input() customerId
  events: any[];
  propeties: string[];


  constructor(private customerService: CustomerService) { }

  displayedColumns: string[];
  columnsToDisplay: string[];
  valores: KeyValue[]
  ngOnInit() {
    this.customerService.getHistory(this.customerId).subscribe(
      events => {
        this.events = events.filter(x => delete x['id'])


        this.displayedColumns = Object.keys(this.events[0])
        this.columnsToDisplay = Object.keys(this.events[0])

        events.forEach(element => {
          this.displayedColumns.forEach(c => {

            console.log(c, element[c])
            //this.valores.push(c, element[c]);
          })
      });

        console.log(this.valores);
      },
      error => console.log(error.map)
    );
  }

  ativarSegundaTabela(id) {

  }
}

export class KeyValue
{
  constructor(id, value) {
    this.id = id;
    this.value = value;
  }
  id: string;
  value: string;
}
