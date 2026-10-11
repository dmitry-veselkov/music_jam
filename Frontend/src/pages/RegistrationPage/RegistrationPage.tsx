import Layout from "@/shared/ui/Layout/Layout.tsx";
import Header from "@/widgets/Header/Header.tsx";
import RegisterForm from "@/features/auth/ui/RegisterForm/RegisterForm.tsx";

const RegistrationPage = () => {
  return (
    <Layout>
      <Header showAuthButtons={false} />
      <RegisterForm />
    </Layout>
  );
};

export default RegistrationPage;