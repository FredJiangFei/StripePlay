import { useEffect, useState } from 'react';

import { Elements } from '@stripe/react-stripe-js';
import CheckoutForm from './CheckoutForm';

function Payment(props) {
  const { stripePromise } = props;
  const [clientSecret, setClientSecret] = useState('');

  useEffect(() => {
    fetch('/create-payment-intent')
      .then((res) => res.json())
      .then(({ clientSecret }) => setClientSecret(clientSecret));
  }, []);

  const appearance = {
    theme: 'stripe',
  };

  return (
    <>
      <h1>Payment</h1>
      {clientSecret && stripePromise && (
        <Elements stripe={stripePromise} options={{ clientSecret, appearance }}>
          <CheckoutForm />
        </Elements>
      )}
    </>
  );
}

export default Payment;
