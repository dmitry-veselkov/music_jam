import Layout from "@/shared/ui/Layout/Layout.tsx";
import Header from "@/widgets/Header/Header.tsx";
import LoginForm from "@/features/auth/ui/LoginForm/LoginForm.tsx";

const LoginPage = () => {
  return (
    <Layout>
      <Header showAuthButtons={false} />
      <LoginForm />
    </Layout>
  );
};

export default LoginPage;