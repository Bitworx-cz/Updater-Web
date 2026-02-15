const APP_URL =
  process.env.NEXT_PUBLIC_UPDATER_APP_URL || "https://app.example.com";

export function ProductSection() {
  return (
    <section id="product" className="section page-grid">
      <div className="flex flex-col gap-4">
        <p className="section-heading">What the Updater app does</p>
        <div className="flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <div>
            <h2 className="section-title">
              A web app for uploading firmware and tracking devices.
            </h2>
            <p className="section-body mt-4">
              The Updater web app lets you upload compiled firmware, organise it
              into projects, and see which devices run which version. Devices
              use a small Arduino library and an access token to check for new
              binaries.
            </p>
          </div>
          <ul className="mt-2 space-y-2 text-sm text-slate-300 md:w-72">
            <li>• Upload firmware manually from your browser.</li>
            <li>• Group devices and binaries under named projects.</li>
            <li>• See device details such as MAC address, chip type and version.</li>
          </ul>
        </div>
      </div>
      <div className="grid gap-4 md:grid-cols-3">
        {[
          {
            title: "Firmware uploads",
            body: "Use the Upload page or your pipeline to send compiled binaries to the service."
          },
          {
            title: "Projects and tokens",
            body: "Projects help you organise binaries and devices. Tokens let devices authenticate update checks."
          },
          {
            title: "Device overview",
            body: "The Devices view shows which firmware version each board is running, by MAC address and chip type."
          }
        ].map((item) => (
          <div key={item.title} className="card">
            <h3 className="mb-2 text-sm font-semibold text-slate-50">
              {item.title}
            </h3>
            <p className="text-sm text-slate-300">{item.body}</p>
          </div>
        ))}
      </div>
    </section>
  );
}

export function TeamsSection() {
  return (
    <section id="teams" className="section page-grid">
      <div className="grid gap-8 md:grid-cols-[minmax(0,1.4fr),minmax(0,1fr)] md:items-start">
        <div>
          <p className="section-heading">Who it&apos;s for</p>
          <h2 className="section-title">Useful for small projects and growing fleets.</h2>
          <p className="section-body mt-4">
            Updater is aimed at people who are already building and shipping
            firmware and want a central place to upload binaries and see which
            devices are on which version.
          </p>
        </div>
        <div className="grid gap-4 text-sm text-slate-300">
          <div className="card">
            <p className="text-xs font-semibold uppercase tracking-wide text-amber-300">
              Individuals &amp; hobby projects
            </p>
            <p className="mt-2">
              Keep a few boards up to date without manually flashing each one.
              Upload a binary once, let the devices call home for updates.
            </p>
          </div>
          <div className="card">
            <p className="text-xs font-semibold uppercase tracking-wide text-emerald-400">
              Small teams
            </p>
            <p className="mt-2">
              Share projects, firmware builds and device information between
              team members instead of relying on local files and spreadsheets.
            </p>
          </div>
          <div className="card">
            <p className="text-xs font-semibold uppercase tracking-wide text-amber-400">
              Larger installations
            </p>
            <p className="mt-2">
              Use projects and tokens to structure multiple device groups and
              firmware versions in a way that matches how you deploy today.
            </p>
          </div>
        </div>
      </div>
      <div className="grid gap-4 md:grid-cols-2">
        <div className="rounded-2xl border border-emerald-500/30 bg-emerald-500/5 p-5 text-sm text-emerald-50">
          <p className="text-xs font-semibold uppercase tracking-wide text-emerald-300">
            For individuals &amp; small teams
          </p>
          <p className="mt-2 text-slate-100">
            Start with one or two projects and a handful of devices. You can
            grow the structure over time as your needs change.
          </p>
        </div>
        <div className="rounded-2xl border border-amber-300/40 bg-amber-300/5 p-5 text-sm text-amber-50">
          <p className="text-xs font-semibold uppercase tracking-wide text-amber-200">
            For bigger fleets
          </p>
          <p className="mt-2 text-slate-100">
            Multiple projects, several firmware versions and many devices can be
            managed from the same web interface.
          </p>
        </div>
      </div>
    </section>
  );
}

export function HowItWorksSection() {
  return (
    <section id="how-it-works" className="section page-grid">
      <div className="flex flex-col gap-3">
        <p className="section-heading">How it works</p>
        <h2 className="section-title">
          From firmware build to device in a few clear steps.
        </h2>
        <p className="section-body">
          The Updater flow mirrors what you already do today: build firmware,
          upload it, and let devices call home for updates using an access
          token.
        </p>
      </div>
      <ol className="grid gap-4 md:grid-cols-3">
        {[
          {
            label: "1 · Prepare",
            title: "Sign in and get a token",
            body: "Use the Updater web app to log in and generate an access token for your account or project."
          },
          {
            label: "2 · Integrate",
            title: "Install the Arduino library",
            body: "Add the Updater library to your sketch and call the update function with your token and firmware name."
          },
          {
            label: "3 · Upload",
            title: "Upload firmware and let devices check in",
            body: "Build your firmware, upload the binary in the app, and let devices periodically ask the service for newer versions."
          }
        ].map((step) => (
          <li key={step.label} className="card">
            <p className="text-xs font-semibold uppercase tracking-wide text-amber-300">
              {step.label}
            </p>
            <h3 className="mt-2 text-sm font-semibold text-slate-50">
              {step.title}
            </h3>
            <p className="mt-2 text-sm text-slate-300">{step.body}</p>
          </li>
        ))}
      </ol>
    </section>
  );
}

export function SocialProofSection() {
  return (
    <section className="section page-grid">
      <div className="grid gap-8 md:grid-cols-[minmax(0,1.4fr),minmax(0,1fr)] md:items-center">
        <div>
          <p className="section-heading">Typical usage</p>
          <h2 className="section-title">A simple place to keep track of firmware.</h2>
          <p className="section-body mt-4">
            Many users start with a handful of devices and a single project:
            they upload firmware after each important build and let boards
            update themselves instead of reflashing manually.
          </p>
        </div>
        <div className="card text-sm text-slate-200">
          <p className="text-xs font-semibold uppercase tracking-wide text-emerald-300">
            Example
          </p>
          <p className="mt-3">
            A small team has several ESP32-based boards in the lab and a few in
            the field. They build firmware in their existing tools, upload
            binaries to Updater, and rely on the device library and token to
            fetch newer versions when available.
          </p>
        </div>
      </div>
    </section>
  );
}

export function PricingSection() {
  return (
    <section id="pricing" className="section page-grid">
      <div className="flex flex-col gap-3">
        <p className="section-heading">Pricing</p>
        <h2 className="section-title">
          Simple tiers as your usage grows.
        </h2>
        <p className="section-body">
          These example tiers describe how you might think about using Updater
          as your number of devices increases. Exact limits and pricing are
          configured in the app.
        </p>
      </div>
      <div className="grid gap-4 md:grid-cols-3">
        <div className="card flex flex-col justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wide text-slate-400">
              Starter
            </p>
            <p className="mt-2 text-3xl font-semibold text-slate-50">$0</p>
            <p className="text-xs text-slate-400">for up to 50 devices</p>
            <ul className="mt-4 space-y-1 text-sm text-slate-300">
              <li>• 1 project</li>
              <li>• Manual rollouts</li>
              <li>• Email support</li>
            </ul>
          </div>
          <a
            href={`${APP_URL}/get-started`}
            className="btn-secondary mt-4 w-full text-center"
          >
            Get started in the app
          </a>
        </div>
        <div className="card relative flex flex-col justify-between border-amber-300/60">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wide text-amber-300">
              Standard
            </p>
            <p className="mt-2 text-3xl font-semibold text-slate-50">
              Usage based
            </p>
            <p className="text-xs text-slate-400">
              Suitable for teams with more devices and projects. Billing can be
              based on device count or traffic.
            </p>
            <ul className="mt-4 space-y-1 text-sm text-slate-300">
              <li>• Unlimited projects</li>
              <li>• Scheduled &amp; progressive rollouts</li>
              <li>• SSO &amp; role-based access</li>
              <li>• Priority support</li>
            </ul>
          </div>
          <a
            href={`${APP_URL}/get-started`}
            className="btn-primary mt-4 w-full text-center"
          >
            Open Updater app
          </a>
        </div>
        <div className="card flex flex-col justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wide text-amber-400">
              Enterprise
            </p>
            <p className="mt-2 text-3xl font-semibold text-slate-50">
              Custom
            </p>
            <p className="text-xs text-slate-400">
              For regulated or complex environments.
            </p>
            <ul className="mt-4 space-y-1 text-sm text-slate-300">
              <li>• Dedicated environment options</li>
              <li>• Advanced governance &amp; audit</li>
              <li>• Implementation support</li>
            </ul>
          </div>
          <a
            href={`${APP_URL}/get-started`}
            className="btn-secondary mt-4 w-full text-center"
          >
            Talk to our team
          </a>
        </div>
      </div>
    </section>
  );
}

export function FAQSection() {
  const faqs = [
    {
      q: "Do I need a lot of devices to benefit from Updater?",
      a: "No. Updater can be useful even if you only have a few boards. It becomes more valuable as the number of devices and firmware versions increases."
    },
    {
      q: "How do devices authenticate to the service?",
      a: "Devices use an access token generated in the Updater web app. The token is passed to the Arduino library, which uses it when checking for new firmware."
    },
    {
      q: "What about security and compliance?",
      a: "Updater is built so that firmware downloads are tied to tokens and projects. How you store and rotate tokens, and how restrictive you make them, is up to you."
    },
    {
      q: "Can I try this with test boards only?",
      a: "Yes. You can start by registering test devices and uploading firmware just for them before using the same flow for boards in the field."
    }
  ];

  return (
    <section id="faq" className="section page-grid">
      <div className="flex flex-col gap-3">
        <p className="section-heading">Questions</p>
        <h2 className="section-title">Common questions.</h2>
        <p className="section-body">
          Here are a few practical points that often come up when people start
          using the Updater web app with their own devices.
        </p>
      </div>
      <div className="grid gap-4 md:grid-cols-2">
        {faqs.map((item) => (
          <div key={item.q} className="card">
            <h3 className="text-sm font-semibold text-slate-50">{item.q}</h3>
            <p className="mt-2 text-sm text-slate-300">{item.a}</p>
          </div>
        ))}
      </div>
    </section>
  );
}

export function DemoCTASection() {
  return (
    <section id="get-started" className="page-grid pb-24 pt-4">
      <div className="card flex flex-col gap-6 border-amber-300/40 bg-slate-900/80">
        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div>
            <p className="section-heading">Next step</p>
            <h2 className="section-title text-2xl md:text-3xl">
              Continue in the Updater web app.
            </h2>
            <p className="section-body mt-3 text-sm md:text-base">
              Log in, generate your access token, install the Arduino library,
              and upload firmware directly in the app. The landing page is just
              the overview—your actual workflows live in the Updater web
              experience.
            </p>
          </div>
          <div className="mt-4 flex flex-col gap-3 md:mt-0 md:min-w-[260px]">
            <a
              href={`${APP_URL}/get-started`}
              className="btn-primary w-full"
            >
              Open Updater web app
            </a>
            <p className="text-xs text-slate-400">
              Prefer reading first? Explore the in-app documentation and
              examples for uploads, projects, and device views.
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}

export function Footer() {
  return (
    <footer className="border-t border-slate-800 bg-slate-950">
      <div className="mx-auto flex w-full max-w-6xl flex-col gap-4 px-4 py-6 text-xs text-slate-500 md:flex-row md:items-center md:justify-between md:px-6">
        <div className="flex items-center gap-2">
          <img
            src="/logo.svg"
            alt="bitworx"
            className="h-4 w-auto"
          />
          <p>© {new Date().getFullYear()} bitworx · Updater. All rights reserved.</p>
        </div>
        <div className="flex flex-wrap items-center gap-4">
          <a href="#top" className="hover:text-slate-300">
            Back to top
          </a>
          <a href="#pricing" className="hover:text-slate-300">
            Pricing
          </a>
          <a href="#faq" className="hover:text-slate-300">
            FAQ
          </a>
        </div>
      </div>
    </footer>
  );
}



