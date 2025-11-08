export interface ModalRef<T = any> {
  close: (result?: T) => void;
  config: {
    data?: any;
  };
}
