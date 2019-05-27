export class Customer {
    constructor(
        public id: string,
        public firstName: string,
        public lastName: string,
        public email: string,
        public birthDate: Date,
        public street: string,
        public number: string,
        public city: string,
        public zipCode: string
    ) {}
}
