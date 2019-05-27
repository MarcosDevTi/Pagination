export class Grid<T> {
        constructor(items: T[], head: any, totalItems: number) {
        this.items = items;
        this.head = head;
        this.totalItems = totalItems;
    }
    items: T[];
    head: any;
    totalItems: number;
}
