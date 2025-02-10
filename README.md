# Yappertron

Yappertron is a dynamic video creation tool that combines text-to-speech (TTS) technology with video processing. It takes multiple text documents (containing stories, scripts, or any narrative content) and a background video, generates TTS audio from the texts, overlays the audio onto the video, and trims the video to match the length of the audio. This makes it easy to produce narrated videos quickly and efficiently.

# Azure Speech Synthesizer Credentials and Environment Variable Setup

This guide explains how to obtain your Azure Speech Synthesizer credentials and configure the required environment variables on Windows. These steps are essential for authenticating your application with the Azure Speech Service.

---

## Part 1: Obtaining Your Azure Speech Synthesizer Credentials

### 1. Log in to the Azure Portal

- Open your web browser and navigate to [https://portal.azure.com](https://portal.azure.com).
- Sign in with your Azure account credentials.

### 2. Create a Speech Resource

- Click **Create a resource**.
- In the search bar, type **Speech** and select the **Speech** service.
- Click **Create**.
- Fill in the required details:
  - **Subscription:** Choose your Azure subscription.
  - **Resource Group:** Select an existing group or create a new one.
  - **Region:** Choose the region where you want your Speech resource to be hosted (e.g., `westus`).
  - **Pricing Tier:** Select the appropriate tier (e.g., Free Tier F0 or Standard S0).
- Click **Review + create**, then click **Create** to deploy the resource.

### 3. Retrieve the Credentials

- Once your Speech resource is deployed, go to the resource's **Keys and Endpoint** section.
- **Copy one of the keys** (either Key1 or Key2); this will be your `SPEECH_KEY`.
- Note the **Location/Region** (for example, `westus`); this will be your `SPEECH_REGION`.

---

## Part 2: Setting Environment Variables on Windows

Your application will use the `SPEECH_KEY` and `SPEECH_REGION` environment variables to authenticate with the Azure Speech Service.

1. **Open Command Prompt as Administrator:**
   - Click **Start**, type `cmd`, right-click **Command Prompt**, and choose **Run as administrator**.

2. **Set the Environment Variables:**

   ```cmd
   setx SPEECH_KEY "your-speech-key"
   setx SPEECH_REGION "your-speech-region"
   ```
