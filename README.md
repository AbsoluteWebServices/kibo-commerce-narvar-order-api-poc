## Set up project with JetBrains Rider

1. Open JetBrains Rider IDE.
2. On the Welcome screen, click **Open**.
3. Navigate to the existing project directory and click **Open**.
4. Rider will automatically load and configure your project.

## Environment Variables

Source each value from the Kibo and Narvar environments.

The .env.example file should be copied to .env and values filled in.

## Creating / Configuring the app in Kibo Dev Center

Log into Kibo and navigate to the [Dev Center](https://developer.mozu.com/console/app).

Create an application, specify required fields (name, application id, version).

Click on Packages > Behaviors, and include Order Read.

Click on Packages > Events, Add Event Subscription:
- Paste the URL of the app into the URL field (https://yourapp.com/webhooks/kibo)
- Select all of the Order Events (Order Opened, Order Updated, Order Fulfilled, Order Closed) you want to listen for

Click Install from the upper right and select a Tenant to install the app to.

## Resources

[Kibo Event Notifications](https://docs.kibocommerce.com/help/event-notifications-overview)

[Narvar Order API Spec](https://developer.narvar.com/docs/orderapi/spec)

## Callouts

- Does not handle returns
- Does not handle fulfillments
- Does not validate Kibo as the source of the webhook
