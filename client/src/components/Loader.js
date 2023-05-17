import React, { useContext, useEffect, useState } from 'react';
import ServicesContext from '../services/ServicesContext';

const Loader = () => {
    const [loading, setLoading] = useState(false);
    const { httpService } = useContext(ServicesContext);
    
    useEffect(() => {
        httpService.setSetLoadingFunction(setLoading);
    }, [httpService]);

    return (
        <>
            {loading ? <div className="loader-container">
                            <div className="loader">Loading...</div>
                        </div> : <></>}
        </>
    );
};

export default Loader;