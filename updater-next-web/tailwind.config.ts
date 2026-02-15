import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./src/**/*.{js,ts,jsx,tsx,mdx}"],
  theme: {
    extend: {
      colors: {
        // Core brand palette, aligned with the Updater + bitworx identity
        "brand-primary": "#FEFF00", // updater yellow
        "brand-primary-soft": "#FEF9C3", // soft yellow tint
        "brand-secondary": "#030F0F", // deep ink / background
        "brand-accent": "#F97316" // warm accent for highlights if needed
      },
      boxShadow: {
        soft: "0 24px 60px rgba(3, 15, 15, 0.6)"
      },
      backgroundImage: {
        "radial-soft":
          "radial-gradient(circle at top, rgba(254, 255, 0, 0.14), transparent 60%)"
      }
    }
  },
  plugins: []
};

export default config;



