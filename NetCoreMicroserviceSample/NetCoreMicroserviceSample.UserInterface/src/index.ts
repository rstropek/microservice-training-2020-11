import 'bootstrap';
import "./index.css";
import 'bootstrap/dist/css/bootstrap.css';
import { MachineConfigurationViewModel } from './viewModel';
import { HubConnectionBuilder } from '@aspnet/signalr';

interface IProfile {
    name: string;
    email: string;
    subject: string;
}

// Global event listener to start interacting with the DOM/page once it's loaded
document.addEventListener('DOMContentLoaded', async () => {
    // use a view model to get an abstraction of the DOM interaction model
    var viewModel = new MachineConfigurationViewModel();

    // Call Profile Endpoint
    const response = await fetch("/api/auth/profile");
    if (response.ok) {
        var profile: IProfile = await response.json();
        viewModel.profile = profile.name;
    } else {
        viewModel.profile = null;
    }

});
