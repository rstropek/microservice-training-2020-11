import 'bootstrap';
import "./index.css";
import 'bootstrap/dist/css/bootstrap.css';
import { MachineConfigurationViewModel } from './viewModel';
import { HubConnectionBuilder } from '@aspnet/signalr';
import { NetCoreMicroserviceSampleApi } from './apiClient/netCoreMicroserviceSampleApi';
import { MachineSetting, MachineSettingsUpdateDto, MachineSwitch } from './apiClient/models';

interface IProfile {
    name: string;
    email: string;
    subject: string;
}

// Global event listener to start interacting with the DOM/page once it's loaded
document.addEventListener('DOMContentLoaded', async () => {
    try {
        // integrate SignalR
        const hubConnection = new HubConnectionBuilder()
            .withUrl("/livedata")
            .build();

        await hubConnection.start();

        // use a view model to get an abstraction of the DOM interaction model
        var viewModel = new MachineConfigurationViewModel();

        const client = new NetCoreMicroserviceSampleApi({ baseUri: '/' });

        viewModel.selectMachine = async m => {
            console.log(m.name + " selected");

            const imageResponse = await client.getMachineImage(m.id);
            viewModel.machineImage = imageResponse._response.bodyAsText;

            viewModel.settings = <MachineSetting[]><unknown>await client.getMachineSettings(m.id);
            viewModel.switches = <MachineSwitch[]><unknown>await client.getMachineSwitches(m.id);

            hubConnection.stream("MachineData", m.id)
                .subscribe({
                    next: (value) => {
                        viewModel.sensorValue = value;
                    },
                    complete: () => {
                        console.log("complete");
                    },
                    error: (err) => {
                        console.log(err);
                    }
                });
        }

        // react on the save settings button in the view model -> update settings on server via an API call
        viewModel.settingsSaveClicked = async (machine, settings) => {
            console.log("setting save clicked", machine, settings);

            // build server side DTO
            const settingsToUpdate = settings.map(s => <MachineSettingsUpdateDto>{
                id: s.id,
                value: s.value
            });

            await client.updateMachineSettings(machine.id, {
                body: settingsToUpdate
            });
        }

        // react on clicking a switch in the view model
        viewModel.switchClicked = async s => {
            console.log("switch clicked", s);

            await client.setMachineSwitch(s.machineId, s.id);
        }

        const serverResponse = await hubConnection.invoke("hello", "Hello from our machine UI ;)");
        console.log(serverResponse);

        // Call Profile Endpoint
        const response = await fetch("/api/auth/profile");
        if (response.ok) {
            var profile: IProfile = await response.json();
            viewModel.profile = profile.name;
        } else {
            viewModel.profile = null;
        }

        const machines = await client.getAllMachines();
        console.log(machines);

        viewModel.machines = machines;
    } catch (e) {
        console.error('Looks like there was a problem. Status Code: ' + e);
    }
});
