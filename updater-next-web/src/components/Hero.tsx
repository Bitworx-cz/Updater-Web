export function Hero() {
  return (
    <section
      id="top"
      className="relative overflow-hidden border-b border-slate-800 bg-radial-soft"
    >
      <div className="page-grid pt-16 md:pt-20">
        <div className="flex flex-col gap-10 md:flex-row md:items-center md:justify-between">
          <div className="flex max-w-xl flex-col gap-6">
            <div className="pill w-fit">
              <span className="h-2 w-2 rounded-full bg-emerald-400" />
              <span>Built for ops, trusted by engineering</span>
            </div>
            <div className="space-y-4">
              <h1 className="text-balance text-4xl font-semibold tracking-tight text-slate-50 md:text-5xl lg:text-6xl">
                Manage firmware updates for your devices.
              </h1>
              <p className="section-body">
                Use the Updater web app to upload firmware binaries, group
                boards into projects, and let Arduino-based devices check
                periodically for new versions using a simple library call.
              </p>
            </div>
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
              <a
                href="https://app.example.com/get-started"
                className="btn-primary w-full sm:w-auto"
              >
                Open Updater web app
              </a>
              <a
                href="#how-it-works"
                className="btn-secondary w-full sm:w-auto text-slate-200"
              >
                See how it works
              </a>
            </div>
            <p className="text-xs text-slate-400">
              The app lets you sign in, generate an access token, install the
              Arduino library, upload firmware, and see projects and devices in
              one place.
            </p>
          </div>
          <div className="mt-4 w-full max-w-md md:mt-0 md:max-w-lg">
            <div className="card relative">
              <div className="mb-4 flex items-center justify-between text-xs text-slate-400">
                <span>Live rollout snapshot</span>
                <span className="inline-flex items-center gap-1 rounded-full bg-emerald-500/10 px-2 py-1 text-[10px] font-medium text-emerald-300">
                  <span className="h-1.5 w-1.5 rounded-full bg-emerald-400" />
                  Healthy
                </span>
              </div>
              <div className="space-y-3 text-xs">
                <div className="flex items-center justify-between">
                  <span className="text-slate-300">Devices online</span>
                  <span className="font-mono text-amber-200">12,482</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-slate-300">On latest binary</span>
                  <span className="font-mono text-emerald-300">96.4%</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-slate-300">Rollout window</span>
                  <span className="font-mono text-slate-200">03:00–05:00 UTC</span>
                </div>
              </div>
              <div className="mt-6 h-px w-full bg-gradient-to-r from-transparent via-slate-700 to-transparent" />
              <div className="mt-4 grid grid-cols-3 gap-3 text-xs">
                <div className="rounded-xl bg-slate-900/80 p-3">
                  <p className="text-[10px] uppercase tracking-wide text-slate-400">
                    Projects
                  </p>
                  <p className="mt-1 text-lg font-semibold text-slate-50">38</p>
                  <p className="mt-1 text-[11px] text-emerald-300">
                    5 active rollouts
                  </p>
                </div>
                <div className="rounded-xl bg-slate-900/80 p-3">
                  <p className="text-[10px] uppercase tracking-wide text-slate-400">
                    Risk window
                  </p>
                  <p className="mt-1 text-lg font-semibold text-slate-50">
                    &lt; 7 min
                  </p>
                  <p className="mt-1 text-[11px] text-amber-200">
                    Automated rollback
                  </p>
                </div>
                <div className="rounded-xl bg-slate-900/80 p-3">
                  <p className="text-[10px] uppercase tracking-wide text-slate-400">
                    Compliance
                  </p>
                  <p className="mt-1 text-lg font-semibold text-slate-50">
                    100%
                  </p>
                  <p className="mt-1 text-[11px] text-slate-300">
                    Full device audit
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}



