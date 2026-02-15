import { Header } from "@/components/Header";
import { Hero } from "@/components/Hero";
import {
  DemoCTASection,
  FAQSection,
  Footer,
  HowItWorksSection,
  PricingSection,
  ProductSection,
  SocialProofSection,
  TeamsSection
} from "@/components/Sections";

export default function Page() {
  return (
    <>
      <Header />
      <main>
        <Hero />
        <ProductSection />
        <TeamsSection />
        <HowItWorksSection />
        <SocialProofSection />
        <PricingSection />
        <FAQSection />
        <DemoCTASection />
      </main>
      <Footer />
    </>
  );
}



