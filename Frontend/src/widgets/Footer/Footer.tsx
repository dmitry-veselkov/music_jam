import styles from "./Footer.module.css"

const Footer = () => {
  return (
    <footer className={styles.footer}>
      <img
        src="/Footer.svg"
        alt=""
        className={styles.decoration}
      />
    </footer>
  );
};

export default Footer;