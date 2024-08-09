import http from 'k6/http';
import { check } from 'k6';

// Custom function to generate a GUID (Globally Unique Identifier)
function generateGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    }).toUpperCase();
}

// Function to generate a random phone number
function generatePhoneNumber() {
    return `62${Math.floor(Math.random() * 10000000000)}`;
}

// Function to generate a random email
function generateEmail(username) {
    return `${username}@example.com`;
}

// Function to generate a random username (lowercase and numbers)
function generateUsername() {
    const chars = 'abcdefghijklmnopqrstuvwxyz0123456789';
    let username = '';
    for (let i = 0; i < 8; i++) {
        username += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return username;
}

// Function to generate a random name (without numbers)
function generateName() {
    const names = ['Alice', 'Bob', 'Charlie', 'Diana', 'Eve', 'Frank', 'George', 'Hannah', 'Isaac', 'Jack'];
    return names[Math.floor(Math.random() * names.length)];
}

// Function to generate a random business license number
function generateLicenseNumber(index) {
    return `${String(index + 1).padStart(13, '0')}`;
}

// Function to create business licenses
function createBusinessLicenses() {
    return Array.from({ length: 3 }, (_, index) => ({
        licenseNumber: generateLicenseNumber(index),
        name: `Business Operation License ${index + 1}`,
        description: `License to operate industrial automation business ${index + 1}.`
    }));
}

// Function to create staff members
function createStaffMembers() {
    return Array.from({ length: 3 }, (_, index) => {
        const username = generateUsername();
        return {
            username: username,
            phoneNumber: generatePhoneNumber(),
            email: generateEmail(username),
            name: generateName(),
            address: {
                addressLine1: `Jl. Maju Terus No.456`,
                addressLine2: `Blok B`,
                addressLine3: `Lantai 3`,
                city: `Bandung`,
                state: `Jawa Barat`,
                postalCode: `40234`,
                country: `Indonesia`
            },
            timeZoneId: `Asia/Jakarta`
        };
    });
}

// Function to create a payload
function createPayload() {
    const username = generateUsername();
    const email = generateEmail(username);
    const phoneNumber = generatePhoneNumber();
    const identityId = generateGUID();

    return {
        identityId: identityId,
        username: username,
        email: email,
        phoneNumber: phoneNumber,
        timeZoneId: "Asia/Jakarta",
        address: {
            addressLine1: "Jl. Merdeka No.123",
            addressLine2: "Blok A",
            addressLine3: "Lantai 2",
            city: "Jakarta",
            state: "DKI Jakarta",
            postalCode: "10110",
            country: "Indonesia"
        },
        businessName: "Doe Industries",
        contactPerson: "Jane Doe",
        taxId: "123456789012345",
        websiteUrl: "https://www.doeindustries.com",
        businessDescription: "Leading provider of industrial automation solutions.",
        referralCode: "",
        businessLicenses: createBusinessLicenses(),
        staffs: createStaffMembers()
    };
}

// Main function for k6 script
export default function () {
    const payload = createPayload();

    const url = 'https://localhost:7201/api/v1/users/business';
    const params = {
        headers: {
            'Content-Type': 'application/json',
            'X-Idempotency-Key': generateGUID()  // Add the X-Idempotency-Key header with a random GUID
        }
    };

    const res = http.post(url, JSON.stringify(payload), params);
 
    check(res, {
        'is status 201': (r) => r.status === 201,
    });
}
