/*class Test1 {
    async register() {
        try {
            const response = await fetch('/User/MyProfile', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            if (response.ok) {
                // Redirect to the returned view
                window.location.href = response.url;
            } else {
                console.error('Failed to call the server');
            }
        } catch (error) {
            console.error('An error occurred:', error);
        }
    }

}

function openModal() {
    const tester = new Test1();
    tester.register();
}
*/
// scripts.ts
//import { UserDetails } from './models'; // Adjust the path based on your project structure
/*
function openModal(user: UserDetails): void {
    const modal = document.getElementById('myModal')!;
    const userIdInput = document.getElementById('userId') as HTMLInputElement;
    const userEmailInput = document.getElementById('userEmail') as HTMLInputElement;
    const addressInput = document.getElementById('Address') as HTMLInputElement;
    const cityInput = document.getElementById('City') as HTMLInputElement;
    const stateInput = document.getElementById('State') as HTMLInputElement;
    const countryInput = document.getElementById('Country') as HTMLInputElement;
    const phoneNumberInput = document.getElementById('PhoneNumber') as HTMLInputElement;
    const alternatePhoneNumberInput = document.getElementById('AlternatePhoneNumber') as HTMLInputElement;

    // Set user data in the form
    userIdInput.value = user.UserId.toString();
    userEmailInput.value = user.Email;
    addressInput.value = user.Address;
    cityInput.value = user.City;
    stateInput.value = user.State;
    countryInput.value = user.Country;
    phoneNumberInput.value = user.PhoneNumber;
    alternatePhoneNumberInput.value = user.AlternatePhoneNumber;

    // Display the modal
    modal.style.display = 'block';
}

function closeModal(): void {
    const modal = document.getElementById('myModal')!;
    modal.style.display = 'none';
}

function submitForm(): void {
    // Add logic to handle form submission
    // You can use AJAX to send the form data to the server
    // For simplicity, let's just close the modal for now
    closeModal();
}

// Open the modal when the page loads
document.addEventListener('DOMContentLoaded', () => {
    // Assuming you have a user object available
    const user: UserDetails = {
        UserId: 1,
        Email: 'example@email.com',
        Address: '123 Main St',
        City: 'Cityville',
        State: 'Stateville',
        Country: 'Countryland',
        PhoneNumber: '123-456-7890',
        AlternatePhoneNumber: '987-654-3210',
        // Other properties
    };

    // Open the modal with user data
    openModal(user);
});


// models.ts
export interface UserDetails {
    UserId: number;
    Email: string;
    Address: string;
    City: string;
    State: string;
    Country: string;
    PhoneNumber: string;
    AlternatePhoneNumber: string;
    // ... other properties
}
*/
// scripts.ts
/*function openModal(): void {
    const modal = document.getElementById('myModal');

    if (modal) {
        modal.style.display = 'block';
    }
}*/
//# sourceMappingURL=Details.js.map