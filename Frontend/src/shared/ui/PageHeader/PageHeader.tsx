import type { ReactNode } from "react";
import styles from "./PageHeader.module.css"

type PageHeaderProps = {
  title : string,
  children? : ReactNode,
}

const PageHeader = ({ title, children }: PageHeaderProps) => (
  <div className={styles.header}>
    <h1 className={styles.title}>{title}</h1>
    {children}
  </div>
);

export default PageHeader;