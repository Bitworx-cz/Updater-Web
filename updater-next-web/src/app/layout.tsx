import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Updater – Effortless Software Rollouts at Scale",
  description:
    "Updater gives teams a single control plane for safe, auditable software distribution across every device.",
  openGraph: {
    title: "Updater – Effortless Software Rollouts at Scale",
    description:
      "Ship updates with confidence. One pane of glass for devices, projects, and binaries.",
    type: "website",
    images: ["/logo-updater.svg"]
  },
  keywords: [
    "software updater",
    "device management",
    "binary distribution",
    "deployment automation"
  ]
};

export default function RootLayout({
  children
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body>{children}</body>
    </html>
  );
}



