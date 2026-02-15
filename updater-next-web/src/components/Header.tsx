const APP_URL =
  process.env.NEXT_PUBLIC_UPDATER_APP_URL || "https://app.example.com";

const navItems = [
  { label: "Product", href: "#product" },
  { label: "Teams", href: "#teams" },
  { label: "How it works", href: "#how-it-works" },
  { label: "Pricing", href: "#pricing" },
  { label: "FAQ", href: "#faq" }
];

export function Header() {
  return (
    <header className="sticky top-0 z-40 border-b border-slate-800/80 bg-slate-950/80 backdrop-blur">
      <div className="mx-auto flex h-16 w-full max-w-6xl items-center justify-between px-4 md:h-20 md:px-6">
        <a href="#top" className="flex items-center gap-3">
          <img
            src="/logo-updater.svg"
            alt="Updater – device-aware software delivery"
            className="h-7 w-auto md:h-8"
          />
        </a>
        <nav className="hidden items-center gap-8 text-sm text-slate-300 md:flex">
          {navItems.map((item) => (
            <a
              key={item.href}
              href={item.href}
              className="transition hover:text-amber-200"
            >
              {item.label}
            </a>
          ))}
        </nav>
        <div className="flex items-center gap-2">
          <a
            href="#pricing"
            className="hidden text-xs font-medium text-slate-300 md:inline-flex hover:text-amber-200"
          >
            View pricing
          </a>
          <a
            href={APP_URL}
            className="btn-primary text-xs md:text-sm"
          >
            Open Updater app
          </a>
        </div>
      </div>
    </header>
  );
}



