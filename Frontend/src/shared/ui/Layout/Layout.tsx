import styles from "./Layout.module.css";

type LayoutProps = {
  children: React.ReactNode;
}

const Layout = ({ children }: LayoutProps) => {
  return <div className={styles.pageLayout}>{children}</div>;
};

export default Layout;